namespace GUI.Network.Shared
{
    internal class AccessKeys
    {
        private AccessKeys() { }

        private static AccessKeys _instance;

        private static readonly object _lock = new object();

        internal static AccessKeys GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AccessKeys();
                        _instance.ExchangeRates = new KeyPair { AccessKey = "3b196d293fee698416c3e507d63361dd", URI = "http://api.exchangeratesapi.io/v1/" };
                        //_instance.Fixer = new KeyPair { AccessKey = "dde78efb2dd609b21ed7ea0fe1fbf1e8", URI = "http://data.fixer.io/api/" };
                        _instance.Fixer = new KeyPair { AccessKey = "2c534ca232efc544b377a6d0a426d2a7", URI = "http://data.fixer.io/api/" };
                    }
                }
            }

            return _instance;
        }

        public KeyPair ExchangeRates { get; private set; }
        public KeyPair Fixer { get; private set; }
    }
}
