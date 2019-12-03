[System.Serializable]
public class Footstep
{
    public float posX;
    public float posY;
    public float time;
    public float velo;

    public Footstep(float posX, float posY, float time, float velo)
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
