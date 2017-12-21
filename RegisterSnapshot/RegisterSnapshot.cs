using System;

namespace RegisterSnapshot
{
	public class RegisterSnapshot
	{
		private Register[] regs;
		private readonly bool[,] hshakeMatrix;

    //Initialazing registers and handshake matrix
		public RegisterSnapshot()
		{
			this.regs = new Register[2];
			this.hshakeMatrix = new bool[2, 2];
      for (var i = 0; i < 2; ++i)
				this.regs[i] = new Register();
		}

		public int[] Scan(int pid)
		{
			var moved = new bool[2];

			while (true)
			{
				for (var i = 0; i < 2; ++i)
					hshakeMatrix[pid, i] = regs[i].bitmask[pid];
				
        //Creating two clones to check for changes
				var a = CloneRegs();
				var b = CloneRegs();

				var changed = false;
        //Checking for changes
				for (var i = 0; i < 2; ++i)
				{
					if (a[i].bitmask[pid] == b[i].bitmask[pid] &&
						b[i].bitmask[pid] == hshakeMatrix[pid, i] &&
						a[i].toggle == b[i].toggle) continue;

					if (moved[i])
            //Taking a snapshot if one changed twice
						return a[i].snapshot;
            
					moved[i] = true;
					changed = true;
					break;
				}

        //Waiting if not good
				if (changed) continue;

        //Taking a snapshot if all's good
				var snapshot = new int[2];
				for (var i = 0; i < 2; ++i)
					snapshot[i] = a[i].value;
				return snapshot;
			}
		}

    //Updating value, bitmask and switching toggle
		public void Update(int pid, int value)
		{
			var newBitmask = new bool[2];
			for (var i = 0; i < 2; ++i)
				newBitmask[i] = !hshakeMatrix[i, pid];
			var snapshot = Scan(pid);
			
			regs[pid].Update(value, newBitmask, snapshot);
		}

		private Register[] CloneRegs()
		{
			return (Register[])regs.Clone();
		}
	}
}