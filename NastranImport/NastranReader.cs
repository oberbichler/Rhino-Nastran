using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NastranImport
{
    public class NastranReader
    {
        private readonly TextReader _textReader;

        public List<GeometryBase> Geometries { get; private set; } = new List<GeometryBase>();

        private NastranReader(TextReader textReader)
        {
            _textReader = textReader;
            ReadLine();
        }

        public static NastranReader Load(TextReader textReader)
        {
            var reader = new NastranReader(textReader);
            reader.ReadAll();
            return reader;
        }

        private string CurrentLine { get; set; }

        private string ReadLine()
        {
            do
            {
                CurrentLine = _textReader.ReadLine()?.TrimEnd();
            } while (CurrentLine?.StartsWith("$") ?? false);    // skip comments

            return CurrentLine;
        }

        private bool EndOfFile => CurrentLine == null;

        private string RecordType => CurrentLine?.Substring(0, Math.Min(8, CurrentLine.Length)).TrimEnd();

        public void ReadAll()
        {
            while (!EndOfFile)
            {
                if (TryReadBulk())
                    continue;

                ReadLine();
            }
        }

        public bool TryReadBulk()
        {

            if (CurrentLine != "BEGIN BULK")
                return false;

            var vertices = new Dictionary<int, int>();
            var faces = new Dictionary<int, MeshFace>();

            var mesh = new Mesh();

            while (!CurrentLine.StartsWith("ENDDATA"))
            {
                ReadLine();

                if (RecordType == "GRID")
                {
                    var key = CurrentLine.Substring(8, 16).ToInt();

                    var x = CurrentLine.Substring(24, 8).ToDouble();
                    var y = CurrentLine.Substring(32, 8).ToDouble();
                    var z = CurrentLine.Substring(40, 8).ToDouble();

                    var index = mesh.Vertices.Add(x, y, z);

                    vertices.Add(key, index);

                    continue;
                }

                if (RecordType == "GRID*")
                {
                    var key = CurrentLine.Substring(8, 16).ToInt();

                    var x = CurrentLine.Substring(40, 16).ToDouble();
                    var y = CurrentLine.Substring(56, 16).ToDouble();

                    ReadLine();

                    var z = CurrentLine.Substring(8, 16).ToDouble();

                    var index = mesh.Vertices.Add(x, y, z);

                    vertices.Add(key, index);

                    continue;
                }

                if (RecordType == "CTRIA3")
                {
                    var key = CurrentLine.Substring(8, 8).ToInt();

                    var a = int.Parse(CurrentLine.Substring(24, 8));
                    var b = int.Parse(CurrentLine.Substring(32, 8));
                    var c = int.Parse(CurrentLine.Substring(40, 8));

                    faces.Add(key, new MeshFace(a, b, c));

                    continue;
                }

                if (RecordType == "CQUAD4")
                {
                    var key = CurrentLine.Substring(8, 8).ToInt();

                    var a = int.Parse(CurrentLine.Substring(24, 8));
                    var b = int.Parse(CurrentLine.Substring(32, 8));
                    var c = int.Parse(CurrentLine.Substring(40, 8));
                    var d = int.Parse(CurrentLine.Substring(48, 8));

                    faces.Add(key, new MeshFace(a, b, c, d));

                    continue;
                }
            }

            ReadLine();

            foreach (var face in faces.Values)
            {
                var a = vertices[face.A];
                var b = vertices[face.B];
                var c = vertices[face.C];
                var d = vertices[face.D];

                mesh.Faces.AddFace(a, b, c, d);
            }

            Geometries.Add(mesh);

            return true;
        }
    }
}
