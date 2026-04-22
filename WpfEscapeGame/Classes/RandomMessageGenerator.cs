using System;
using System.Collections.Generic;
using System.Text;

namespace WpfEscapeGame.Classes
{
    public static class RandomMessageGenerator
    {
        private static readonly Random _rnd = new Random();

        private static readonly string[] _itemDoesNotFit =
        {
            "That doesn't seem to work.",
            "Nope, that's not going to do anything.",
            "I don't think these two go together."
        };

        private static readonly string[] _itemNotPortable =
        {
            "I can't pick that up, it's too heavy.",
            "That item is fixed in place.",
            "There's no way I'm carrying that around."
        };

        private static readonly string[] _doorWrongKey =
        {
            "This key doesn't fit the lock.",
            "Wrong key. Keep looking.",
            "That key doesn't belong here."
        };

        public static string GetRandomMessage(MessageType t)
        {
            return t switch
            {
                MessageType.ItemDoesNotFit => _itemDoesNotFit[_rnd.Next(_itemDoesNotFit.Length)],
                MessageType.ItemNotPortable => _itemNotPortable[_rnd.Next(_itemNotPortable.Length)],
                MessageType.DoorWrongKey => _doorWrongKey[_rnd.Next(_doorWrongKey.Length)],
                _ => "Something went wrong."
            };
        }
    }
}