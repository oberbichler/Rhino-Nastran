using System;
using System.Diagnostics;
using System.IO;
using Rhino;
using Rhino.FileIO;
using Rhino.PlugIns;
using Rhino.UI;

namespace NastranImport
{
    public class Info : FileImportPlugIn
    {
        public Info()
        {
            Instance = this;
        }

        public static Info Instance
        {
            get; private set;
        }

        protected override FileTypeList AddFileTypes(FileReadOptions options)
        {
            var result = new FileTypeList();
            result.AddFileType("Nastran (*.nas, *.dat)", "nas", "dat");
            return result;
        }

        protected override bool ReadFile(string filename, int index, RhinoDoc doc, FileReadOptions options)
        {
            try
            {
                using (var stream = File.OpenText(filename))
                {
                    var data = NastranReader.Load(stream);

                    foreach (var geometry in data.Geometries)
                        doc.Objects.Add(geometry);
                }

                return true;
            }
            catch (Exception ex) when (!Debugger.IsAttached)
            {
                Dialogs.ShowMessage(ex.Message, "Import Error", ShowMessageButton.OK, ShowMessageIcon.Error);

                return false;
            }
        }
    }
}
