using System;
using Beamable.Common.Interfaces;
using Beamable.Server;

namespace Beamable.Common.Models
{
    [Serializable]
    public class TurnData: StorageDocument, ISetStorageDocument<TurnData>
    {
        public long TurnDataId;
        public long CurrentTurn;
        public long FromPlayer;// Stores the PlayerId of the player whose turn it is

        public void Set(TurnData document)
        {
            TurnDataId = document.TurnDataId;
            CurrentTurn = document.CurrentTurn;
            FromPlayer = document.FromPlayer;
        }

        public void SwitchTurn(long playerId)
        {
            FromPlayer = CurrentTurn;
            CurrentTurn = playerId;
        }
    }
}