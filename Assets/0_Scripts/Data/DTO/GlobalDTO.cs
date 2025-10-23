using UnityEngine;

[SerializeField]
public class GlobalDTO
{
    public int Version;

    public string ProjileId;
    public string GameVersion;

    public long CreatedAt;
    public long UpdatedAt;

    public int Seed;

    public MineSaveDTO Mine;
}