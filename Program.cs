using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace swbf
{
   class Program
   {
      static void Main(string[] args)
      {
         if (args.LongCount() == 0)
         {
            print_usage();

            return;
         }
         
         var directory = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

         var factor = 4u;

         if (args.LongCount() >= 2)
         {
            factor = uint.Parse(args[1]);
         }

         process_directory(directory, factor);
      }

      static void print_usage()
      {
         Console.WriteLine(@"Usage: model-edit <directory> [<factor>]
   All *.model files found in <directory> will have detail information edited.
   The detail information for all modes will be divided by <factor>, it defaults to four.");
      }

      static void process_directory(string directory, uint factor)
      {
         var tasks = new List<Task>();

         foreach (var file in Directory.EnumerateFiles(directory))
         {
            var extension = Path.GetExtension(file);

            if (extension != ".model") continue;

            tasks.Add(Task.Factory.StartNew(() => process_file(file, factor)));
         }

         Task.WaitAll(tasks.ToArray());
      }

      static void process_file(string file_path, uint factor)
      {
         var mapped_file = MemoryMappedFile.CreateFromFile(file_path, FileMode.Open);
         var editor = new Ucfb_editor(mapped_file);

         var model = editor.find_child("modl");
         if (model != null) Modl_editor.edit(model, factor);

         var game_model = editor.find_child("gmod");
         if (game_model != null) Gmod_editor.edit(game_model, factor);

         Console.WriteLine("Edited {0}", file_path);
      }
   }
}
