using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Discussions.Public.Contracts
{
    public interface IDiscussionRemover
    {
        Task RemoveDisscusion(Guid discussionId );    
    }
}
