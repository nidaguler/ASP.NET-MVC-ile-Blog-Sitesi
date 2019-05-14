using NeYemekYapsam.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        protected static DatabaseContext context;
        private static object _LockSync = new object();


        protected RepositoryBase()
        {
            CreateContext();
        }

        private static void CreateContext()
        {
            if (context == null)
            {
                lock (_LockSync)
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }

                }

            }


        }
    }
}
