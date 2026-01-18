using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Domain.Shared
{
    public class CustomSettingModel
    {
        public bool IsDevelopment
        {
            get; set;
        }
        public string DbConnection
        {
            get; set;
        }

        public string SecretKey
        {
            get; set;
        }

        public bool EnableEncryption
        {
            get; set;
        }
       

    }
}
