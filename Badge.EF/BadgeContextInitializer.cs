using System;
using System.Collections.Generic;
using System.Text;

namespace Badge.EF
{
    public static class BadgeContextInitializer
    {
        public static void Initialize(BadgeContext db)
        {
            db.Database.EnsureCreated();
        }
    }
}
