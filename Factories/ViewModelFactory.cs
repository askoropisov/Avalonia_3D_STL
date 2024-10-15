using Avalonia_3D_STL.ViewModels;
using DryIoc;
using System;

namespace Avalonia_3D_STL.Factories
{
    public class ViewModelFactory
    {
        private readonly IContainer _container;

        public ViewModelFactory(IContainer container)
        {
            _container = container;
        }

        public T Create<T>() where T : ViewModelBase
        {
            try
            {
                var instance = _container.Resolve<T>();
                return instance;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
