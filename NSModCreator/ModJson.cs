using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSModCreator
{
    class ModJson
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //LoadPriority
        //RequiredOnClient
        //Scripts
        public string Version { get; set; }

        public ModJson(string name, string description, string version)
        {
            Name = name;
            Description = description;
            Version = version;
        }
    }
}
