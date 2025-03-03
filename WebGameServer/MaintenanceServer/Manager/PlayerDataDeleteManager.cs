namespace MaintenanceServer;

public class PlayerDataDeleteManager : Singleton<PlayerDataDeleteManager>
{
    private List<DeletedPlayerDao> _deletedPlayers = null;

    public void Initialize()
    {

    }

    public void Process()
    {
        DeletePlayerData();
    }

    private void DeletePlayerData()
    {
        if (_deletedPlayers == null || _deletedPlayers.Count == 0)
        {
            var proc = new spSelectDeletedPlayer();
            if (proc.Run())
            {
                _deletedPlayers = proc.deletedPlayers;
            }
        }

        if (_deletedPlayers != null && _deletedPlayers.Count > 0)
        {
            var deletedPlayer = _deletedPlayers[0];
            var proc = new spDeletePlayerData(deletedPlayer);
            proc.Run();

            _deletedPlayers.Remove(deletedPlayer);
        }
    }
}