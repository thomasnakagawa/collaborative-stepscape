[System.Serializable]
public class Footstep
{
    public string posX;
    public string posY;
    public string time;
    public string velo;

    public Footstep(string posX, string posY, string time, string velo)
    {
        this.posX = posX;
        this.posY = posY;
        this.time = time;
        this.velo = velo;
    }

    public override string ToString()
    {
        return "Footstep, posX: " + posX + ", posY: " + posY + ", time: " + time + ", velo: " + velo;
    }
}
