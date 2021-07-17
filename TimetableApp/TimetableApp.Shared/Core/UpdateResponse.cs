using System;

namespace TimetableApp.Core
{
    [Serializable]
    public class UpdateResponse
    {
        public string MD5;
        public string SHA512;
        public string Location;
    }
}
