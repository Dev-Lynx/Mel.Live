using Mel.Live.Models.Entities;
using Mel.Live.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Services.Interfaces
{
    public interface IJwtFactory
    {
        JwtIssuerOptions Options { get; }
        Task<string> GenerateToken(User user);
    }
}
