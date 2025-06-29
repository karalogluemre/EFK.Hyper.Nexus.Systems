namespace EFK.System.Persistence.Injection.Config.AppByteService
{
    public interface IAesService
    {
        public string Encrypt(string clearText);
        public string Decrypt(string clearText);
    }
}
