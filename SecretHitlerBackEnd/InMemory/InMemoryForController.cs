using BackEnd.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretHitlerBackEnd.InMemory
{
    public static class InMemoryForController
    {


        public static void UpdateFrienshipsCache(this IMemoryCache _cache,List<Friendship> friendships)
        {
            _cache.Set("friendships", friendships);
        }
        public static void UpdateUsersCache(this IMemoryCache _cache, List<User> users)
        {
            _cache.Set("users", users);
        }
        public static void UpdateCacheRooms(this IMemoryCache _cache, List<Room> rooms)
        {
            _cache.Set("rooms", rooms);
        }

   
    }
}
