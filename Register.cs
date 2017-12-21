
namespace RegisterSnapshot
{
  //Register flesh and bones
  public class Register
  {
    public int value;
    public bool[] bitmask;
    public bool toggle;
    public int[] snapshot;

    public Register()
    {
      value = 0;
      bitmask = new bool[2];
      toggle = false;
      snapshot = new int[2];
    }

    public void Update(int value, bool[] bitmask, int[] snapshot)
    {
      this.value = value;
      this.bitmask = bitmask;
      this.snapshot = snapshot;
      this.toggle = !toggle;
    }
  }
}