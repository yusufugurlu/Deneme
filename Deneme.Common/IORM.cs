using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deneme.Common
{
   public interface IORM<T> where T:class,new()
    {
        Result<List<T>> Select();
        Result<bool> Insert(T Entity);
        Result<bool> Update(T Entity);
        Result<bool> Delete(T Entity);
    }
}
