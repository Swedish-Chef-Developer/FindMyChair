﻿using FindMyChair.Models.Meetings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FindMyChair.Client
{
	public interface IAAClient
	{
		Task<IEnumerable<Meeting>> GetMeetingsList();
	}
}
