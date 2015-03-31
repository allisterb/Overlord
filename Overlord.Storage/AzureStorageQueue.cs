using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;
using System.IdentityModel.Services;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using AutoMapper;
using Newtonsoft.Json;

using Overlord.Security;

using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;
using Overlord.Storage.Common;

namespace Overlord.Storage
{
    public partial class AzureStorage
    {
        public IEnumerable<CloudQueueMessage> GetDigestMessages(int m)
        {
            return this.DigestQueue.GetMessages(m);

        }

        public async Task<IEnumerable<CloudQueueMessage>> GetDigestMessagesAsync(CancellationToken token)
        {
            return await this.DigestQueue.GetMessagesAsync(32, token);

        }
    }
}
