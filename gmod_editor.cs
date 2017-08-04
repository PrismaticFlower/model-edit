using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace swbf
{
   class Gmod_editor
   {
      static public void edit(Ucfb_editor game_model, uint factor)
      {
         game_model.read_child(); // NAME chunk
         game_model.read_child(); // INFO chunk

         while (!game_model.at_end)
         {
            var child = game_model.read_child();

            child.seek(-4, SeekOrigin.End, false);

            var current_count = child.read_uint32();
            child.seek(-4, SeekOrigin.Current, false);
            child.write_uint32(current_count / factor);
         }
      }

   }
}
