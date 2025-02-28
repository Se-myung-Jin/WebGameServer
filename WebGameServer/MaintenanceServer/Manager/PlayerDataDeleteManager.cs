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
        // 가져와서
        if (_deletedPlayers == null || _deletedPlayers.Count == 0)
        {
            
        }

        // 삭제하기
        if (_deletedPlayers != null && _deletedPlayers.Count > 0)
        {

        }
    }
}