using System;


namespace WW_SYSTEM
{
	[Flags]
	public enum SearchFlags
	{
		// Token: 0x04000014 RID: 20
		NAME = 1,
		// Token: 0x04000015 RID: 21
		AUTHOR = 2,
		// Token: 0x04000016 RID: 22
		ID = 4,
		// Token: 0x04000017 RID: 23
		DESCRIPTION = 8,
		// Token: 0x04000018 RID: 24
		VERSION = 16
	}
}
