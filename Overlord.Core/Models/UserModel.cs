using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Overlord.Core
{
    [DataContract]
    public class UserModel
    {
        #region Public properties
        [DataMember]
        public Guid Id { get; set; }

        public string ETag { get; set; }

        public string Token { get; set; }

        [DataMember]        
        public string UserName { get; set; }

        [DataMember]
        public IList<Guid> Devices { get; set; }
        #endregion
    }
}