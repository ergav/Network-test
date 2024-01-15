
public enum PacketID
{
    S_welcome = 1,
    S_spawnPlayer = 2,
    S_playerPosition = 3,
    S_playerRotation = 4,
    S_playerShoot = 5,
    S_playerDisconnected = 6,
    S_playerHealth = 7,
    S_playerDead = 8,
    S_playerRespawned = 9,


    C_spawnPlayer = 126,
    C_welcomeReceived = 127,
    C_playerMovement = 128,
    C_playerShoot = 129,
    C_playerHit = 130,
}