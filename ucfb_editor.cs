using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace swbf
{
   class Ucfb_editor
   {
      public Ucfb_editor(MemoryMappedFile file)
      {
         _file = file;

         using (var reader = new BinaryReader(_file.CreateViewStream()))
         {
            _identifier = Encoding.UTF8.GetString(reader.ReadBytes(4));
            _size = reader.ReadUInt32();
         }

         _stream = _file.CreateViewStream(8, _size);
         _reader = new BinaryReader(_stream);
         _writer = new BinaryWriter(_stream);

         _offset = 8;
      }

      public string identifier
      {
         get { return _identifier; }
      }

      public long size
      {
         get { return _size; }
      }

      public long position
      {
         get { return _stream.Position; }
         set { seek(value, SeekOrigin.Begin, false); }
      }

      public bool at_end
      {
         get { return !(_stream.Position < _size); }
      }

      public void seek(long amount, SeekOrigin origin, bool aligned)
      {
         _stream.Seek(amount, origin);

         if (aligned)
         {
            var remainder = _stream.Position % 4;

            if (remainder != 0) _stream.Position += (4 - remainder);

         }
      }

      public Ucfb_editor read_child()
      {
         var child_indentifer = _reader.ReadBytes(4);
         var child_size = _reader.ReadUInt32();
         var child_offset = _offset + _stream.Position;

         seek(child_size, SeekOrigin.Current, true);

         return new Ucfb_editor(_file, child_indentifer,
            child_offset, child_size);
      }

      public Ucfb_editor find_child(string identifier)
      {
         var position = _stream.Position;

         while (!at_end)
         {
            var child = read_child();

            if (child.identifier == identifier) return child;
         }

         _stream.Position = position;

         return null;
      }

      public uint read_uint32()
      {
         return _reader.ReadUInt32();
      }

      public void write_uint32(uint value)
      {
         _writer.Write(value);
      }

      Ucfb_editor(MemoryMappedFile file, byte[] identifier, long offset, long size)
      {
         _file = file;

         _stream = _file.CreateViewStream(offset, size);
         _reader = new BinaryReader(_stream);
         _writer = new BinaryWriter(_stream);

         _identifier = Encoding.UTF8.GetString(identifier);
         _size = size;
         _offset = offset;
      }

      string _identifier;
      long _size;
      long _offset = 0;

      MemoryMappedFile _file;
      MemoryMappedViewStream _stream;
      BinaryReader _reader;
      BinaryWriter _writer;
   }
}
