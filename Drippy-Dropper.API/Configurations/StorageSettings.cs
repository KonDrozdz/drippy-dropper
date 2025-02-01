namespace Drippy_Dropper.API.Configurations
{
    namespace Drippy_Dropper.API.Configurations
    {
        public class StorageSettings
        {
            public string BasePath { get; set; }

            public static StorageSettings Initialize()
            {
                var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CloudStorage");
                basePath = Path.GetFullPath(basePath);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                return new StorageSettings { BasePath = basePath };
            }

            public string GetUserBasePath(Guid userId)
            {
                var userBasePath = Path.Combine(BasePath, userId.ToString());
                if (!Directory.Exists(userBasePath))
                {
                    Directory.CreateDirectory(userBasePath);
                }
                return userBasePath;
            }
        }
    }
}