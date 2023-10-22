﻿using CodeBase.Gameplay.Services.Gravity;
using CodeBase.Gameplay.Services.Spawner;
using CodeBase.Gameplay.Services.Spawners.Apples;
using CodeBase.Infrastructure.Factories.Apples;
using CodeBase.Infrastructure.Factories.Characters;
using CodeBase.Infrastructure.Factories.Characters.Camera;
using CodeBase.Infrastructure.Factories.Joysticks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure.Installers
{
    public class LevelInstaller : LifetimeScope
    {
        [SerializeField] private GameObject _attractive;
        [SerializeField] private Rigidbody _attraction;
        
        protected override void Configure(IContainerBuilder builder)
        {
            BindServices(builder);
            BindFactories(builder);
        }

        private void BindServices(IContainerBuilder builder)
        {
            builder
                .Register<LevelSpawner>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            builder
                .Register<IGravityAttraction, GravityAttraction>(Lifetime.Singleton)
                .WithParameter(_attractive)
                .WithParameter(_attraction);
            
            builder
                .Register<IAppleSpawner, AppleSpawner>(Lifetime.Singleton)
                .WithParameter(_attractive);
        }

        private void BindFactories(IContainerBuilder builder)
        {
            builder.Register<IJoystickFactory, JoystickFactory>(Lifetime.Singleton);
            builder.Register<ICharacterFactory, CharacterFactory>(Lifetime.Singleton);
            builder.Register<ICameraFactory, CameraFactory>(Lifetime.Singleton);
            builder.Register<IAppleFactory, AppleFactory>(Lifetime.Singleton);
        }
    }
}