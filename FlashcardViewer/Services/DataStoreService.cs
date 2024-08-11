using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardViewer.Services
{
    public interface IDataStoreService
    {
        IFlashcardDataStore GetDataStore();
        void SetUserLoggedIn(bool isLoggedIn);
    }

    class DataStoreService : IDataStoreService
    {
        private readonly IFlashcardDataStore _localDataStore;
        private readonly IFlashcardDataStore _cloudDataStore;
        private bool _isUserLoggedIn;

        public DataStoreService(IFlashcardDataStore localDataStore, IFlashcardDataStore cloudDataStore)
        {
            _localDataStore = localDataStore;
            _cloudDataStore = cloudDataStore;
        }

        public IFlashcardDataStore GetDataStore()
        {
            return _isUserLoggedIn ? _cloudDataStore : _localDataStore;
        }

        public void SetUserLoggedIn(bool isLoggedIn)
        {
            _isUserLoggedIn = isLoggedIn;
        }
    }
}
