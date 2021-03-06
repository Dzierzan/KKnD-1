#region Copyright & License Information

/*
 * Copyright 2007-2021 The OpenKrush Developers (see AUTHORS)
 * This file is part of OpenKrush, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

namespace OpenRA.Mods.OpenKrush.Mechanics.Researching.LobbyOptions
{
	using System;
	using System.Collections.Generic;
	using OpenRA.Traits;
	using Traits;

	[Desc("Selectable max tech level in lobby.")]
	public class TechLimitInfo : TraitInfo, ILobbyOptions
	{
		public const string Id = "TechLimit";

		public int MaxTechLevel;

		IEnumerable<LobbyOption> ILobbyOptions.LobbyOptions(Ruleset rules)
		{
			var values = new Dictionary<string, string>();

			foreach (var actorInfo in rules.Actors.Values)
			{
				var techLevelBuildableInfo = actorInfo.TraitInfoOrDefault<TechLevelBuildableInfo>();

				if (techLevelBuildableInfo == null)
					continue;

				MaxTechLevel = Math.Max(MaxTechLevel, techLevelBuildableInfo.Level);
			}

			// We start at 1, otherwise building a research building makes no sense!
			for (var i = 1; i <= MaxTechLevel; i++)
				values.Add(i.ToString(), i.ToString());

			yield return new LobbyOption(
				TechLimitInfo.Id,
				"Limit",
				"Maximum tech level.",
				true,
				0,
				new ReadOnlyDictionary<string, string>(values),
				MaxTechLevel.ToString(),
				false,
				ResearchUtils.LobbyOptionsCategory);
		}

		public override object Create(ActorInitializer init)
		{
			return new TechLimit(this);
		}
	}

	public class TechLimit : INotifyCreated
	{
		private readonly TechLimitInfo info;
		public int Limit { get; private set; }

		public TechLimit(TechLimitInfo info)
		{
			this.info = info;
		}

		void INotifyCreated.Created(Actor self)
		{
			Limit = int.Parse(self.World.LobbyInfo.GlobalSettings.OptionOrDefault(TechLimitInfo.Id, info.MaxTechLevel.ToString()));
		}
	}
}
