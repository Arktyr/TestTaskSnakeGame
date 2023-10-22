﻿using CodeBase.Common.Factories;
using CodeBase.Gameplay.Characters;
using CodeBase.Gameplay.Characters.Config;
using CodeBase.Gameplay.Input.Joysticks;
using CodeBase.Gameplay.Services.Gravity;
using CodeBase.Infrastructure.Services.AddressablesLoader.Addresses.Gameplay.Character;
using CodeBase.Infrastructure.Services.AddressablesLoader.Loader;
using CodeBase.Infrastructure.Services.Providers.CharacterProvider;
using CodeBase.Infrastructure.Services.Providers.JoystickProvider;
using CodeBase.Infrastructure.Services.Providers.StaticDataProvider;
using Codice.Client.BaseCommands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure.Factories.Characters
{
    public class CharacterFactory : Factory, ICharacterFactory
    {
        private readonly IGravityAttraction _gravityAttraction;
        private readonly CharacterAddresses _characterAddresses;
        private readonly IJoystickProvider _joystickProvider;
        private readonly ICharacterProvider _characterProvider;

        private readonly CharacterConfig _characterConfig;
        private readonly AssetReferenceGameObject _emptyObject;

        private Transform _root;
        
        public CharacterFactory(IObjectResolver objectResolver, 
            IAddressablesLoader addressablesLoader,
            IGravityAttraction gravityAttraction,
            IJoystickProvider joystickProvider,
            ICharacterProvider characterProvider,
            IStaticDataProvider staticDataProvider) : base(objectResolver, addressablesLoader)
        {
            _gravityAttraction = gravityAttraction;
            _joystickProvider = joystickProvider;
            _characterProvider = characterProvider;

            _characterAddresses = staticDataProvider.AllAssetsAddresses.AllGameplayAddresses.DynamicObjectsAddresses
                .CharacterAddresses;
            _characterConfig = staticDataProvider.GameBalanceData.CharacterConfig;
            _emptyObject = staticDataProvider.AllAssetsAddresses.EmptyObject;
        }
        
        public override async UniTask WarmUp()
        {
            await _addressablesLoader.LoadGameObject(_characterAddresses.Head);
            await _addressablesLoader.LoadGameObject(_emptyObject);
        }

        public async UniTask<BodyParts> CreateBodyPart()
        {
            GameObject prefab = await _addressablesLoader.LoadGameObject(_characterAddresses.Body);
            GameObject gameObject = _objectResolver.Instantiate(prefab, _root);
            
            return gameObject.GetComponent<BodyParts>();
        }

        public override async UniTask Create()
        {
            if (_root == null)
            {
                GameObject rootGameObject = await CreateRootGameObject();
                _root = rootGameObject.transform;
            }

            GameObject head = await CreateHead();

            SetupCharacterMovement(head);
            SetupCharacterBody(head);
            
            Character character = SetupCharacter(head);
            _characterProvider.SetCharacter(character);
            
            Rigidbody rigidbody = head.GetComponent<Rigidbody>();
            _gravityAttraction.AddObjectToAttraction(rigidbody);
        }

        private async UniTask<GameObject> CreateHead()
        {
            GameObject prefab = await _addressablesLoader.LoadGameObject(_characterAddresses.Head);

            return _objectResolver.Instantiate(prefab, _root);
        }
        private async UniTask<GameObject> CreateRootGameObject()
        {
            GameObject prefab = await _addressablesLoader.LoadGameObject(_emptyObject);

            GameObject gameObject = _objectResolver.Instantiate(prefab);

            gameObject.name = "Snake";
            return gameObject;
        }

        private void SetupCharacterBody(GameObject gameObject)
        {
            CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();
            _objectResolver.Inject(characterBody);
        }
        
        private Character SetupCharacter(GameObject gameObject)
        {
            Character character = gameObject.GetComponent<Character>();
            _objectResolver.Inject(character);
            character.Initialize();

            return character;
        }

        private void SetupCharacterMovement(GameObject gameObject)
        {
            CharacterMovement characterMovement = gameObject.GetComponent<CharacterMovement>();
            
            Joystick joystick = _joystickProvider.Joystick;
            
            characterMovement.Construct(joystick, _characterConfig.Speed,
                _characterConfig.RotatingSpeed);
        }
    }
}