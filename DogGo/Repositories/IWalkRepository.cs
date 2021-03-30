using DogGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public interface IWalkRepository
    {
        public List<Walk> GetAllWalks();
        public List<Walk> GetWalksByWalkerId(int walkerId);
    }
}
