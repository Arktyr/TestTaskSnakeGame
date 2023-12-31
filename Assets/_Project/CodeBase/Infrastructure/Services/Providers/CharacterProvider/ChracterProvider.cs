﻿using _Project.CodeBase.Gameplay.Characters;

namespace _Project.CodeBase.Infrastructure.Services.Providers.CharacterProvider
{
    public class CharacterProvider : ICharacterProvider
    {
        public Character Character { get; private set; }

        public void SetCharacter(Character character)
            => Character = character;
    }
}