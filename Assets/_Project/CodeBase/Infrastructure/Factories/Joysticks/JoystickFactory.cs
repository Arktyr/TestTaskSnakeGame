using _Project.CodeBase.Gameplay.Input.Joysticks;
using _Project.CodeBase.Infrastructure.Services.AddressablesLoader.Addresses.Gameplay.Joystick;
using _Project.CodeBase.Infrastructure.Services.AddressablesLoader.Loader;
using _Project.CodeBase.Infrastructure.Services.Providers.JoystickProvider;
using _Project.CodeBase.Infrastructure.Services.Providers.StaticDataProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace _Project.CodeBase.Infrastructure.Factories.Joysticks
{
    public class JoystickFactory : Factory, IJoystickFactory
    {
        private readonly IJoystickProvider _joystickProvider;

        private readonly JoystickAddresses _joystickAddresses;
        private readonly AssetReferenceGameObject _canvas;
        
        public JoystickFactory(IObjectResolver objectResolver, 
            IAddressablesLoader addressablesLoader,
            IJoystickProvider joystickProvider,
            IStaticDataProvider staticDataProvider) 
            : base(objectResolver, addressablesLoader)
        {
            _joystickProvider = joystickProvider;
            
            _joystickAddresses = staticDataProvider.AllAssetsAddresses.AllGameplayAddresses.JoystickAddresses;
            _canvas = staticDataProvider.AllAssetsAddresses.AllGameplayAddresses.Canvas;
        }

        public override async UniTask WarmUp()
        {
            await _addressablesLoader.LoadGameObject(_canvas);
            await _addressablesLoader.LoadGameObject(_joystickAddresses.Joystick);
        }
        
        public override async UniTask Create()
        {
            Canvas canvas = await CreateCanvas();

            Joystick joystick = await CreateJoystick(canvas);
            _joystickProvider.SetJoystick(joystick);
        }
        
        private async UniTask<Canvas> CreateCanvas()
        {
            Canvas prefab = await _addressablesLoader.LoadComponent<Canvas>(_canvas);
            
            return _objectResolver.Instantiate(prefab, null, false);
        }
        
        private async UniTask<Joystick> CreateJoystick(Canvas canvas)
        {
           Joystick prefab = await _addressablesLoader.LoadComponent<Joystick>(_joystickAddresses.Joystick);

            return _objectResolver.Instantiate(prefab, canvas.transform);
        }
    }
}
