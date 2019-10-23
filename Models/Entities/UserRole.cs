using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [CollectionName("Roles")]
    public class UserRole : MongoIdentityRole
    {
        public UserRole() { }
        public UserRole(string name) : base(name) { }
    }
}
