using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace Skateboard3Server.Web.Controllers;

[Route("/skate3/config")]
[ApiController]
public class ConfigController : ControllerBase
{
    [HttpGet("PS3.xml")]
    [Produces("text/xml")]
    public SkateConfig Ps3Config()
    {
        return new SkateConfig
        {
            Reward = 15943774168473329290, //TODO: I think this is a flag
            GameSettings = new GameSettings
            {
                OpenJoinTime0 = 60,
                OpenJoinTime1 = 60,
                OpenJoinTime2 = 45,
                OpenJoinTime3 = 45,
                OpenJoinTime4 = 30,
                OpenJoinTime5 = 30,
                LockedTime = 60,
                MaxGameTime = 600,
                HurryTime = 30,
                ResultsTime = 5,
                RewardsTime = 12,
                ChangeTime = 10,
                TimeoutMS = 5000,
                ProposalTimeout = 30,
                MaxWaitForResetTime = 120,
                InactiveCountThreshold = 2,
                DeathRaceInactiveThreshold = 60.0M
            },
            MatchmakingSettings = new MatchmakingSettings
            {
                AllowDLC = false,
                BypassDirtyCast = false,
                CreateGameDurationMS = 11000,
                FindGameDurationMS = 5000,
                ForceNatWarningMessage = false
            },
            AISchemaUploadsSettings = new AISchemaUploadsSettings
            {
                OnStartCheckSchemaAndReUploadIfRequired = true,
                OnStartAlwaysUpdateAIProfile = true,
                UpdateUploadedAIProfileEveryNTicks = true,
                UpdateUploadedAIProfileEveryNTicks_NumTicks = 216000
            },
            FESettings = new FESettings
            {
                EnableOnlineTipsLoaderData = true,
                EnableFriendsLeaderboardData = true,
                TipsDataNumPagesWaitForOnlineData = 2,
                FriendsLeaderboardDataDelayMS = 2000
            },
            DLCSettings = new DLCSettings
            {
                ProductSettings = new List<ProductSetting>
                {
                    new ProductSetting
                    {
                        ProductID = "FILMERPK00000000",
                        RequiredEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "PLAYER1F00000000",
                        RequiredEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "PLAYER1P00000000",
                            EvenIfInstalled = true,
                            EvenIfPurchased = true
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "PLAYER1P00000000",
                        DisablingEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "PLAYER1F00000000",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        }
                    }
                }
            }
        };
    }

    [HttpGet("XBL2.xml")]
    [Produces("text/xml")]
    public SkateConfig XboxConfig()
    {
        return new SkateConfig
        {
            Reward = 15943774168473329290, //TODO: I think this is a flag
            GameSettings = new GameSettings
            {
                OpenJoinTime0 = 60,
                OpenJoinTime1 = 60,
                OpenJoinTime2 = 45,
                OpenJoinTime3 = 45,
                OpenJoinTime4 = 30,
                OpenJoinTime5 = 30,
                LockedTime = 60,
                MaxGameTime = 600,
                HurryTime = 30,
                ResultsTime = 5,
                RewardsTime = 12,
                ChangeTime = 10,
                TimeoutMS = 5000,
                ProposalTimeout = 30,
                MaxWaitForResetTime = 120,
                InactiveCountThreshold = 2,
                DeathRaceInactiveThreshold = 60.0M
            },
            MatchmakingSettings = new MatchmakingSettings
            {
                AllowDLC = false,
                BypassDirtyCast = false,
                CreateGameDurationMS = 11000,
                FindGameDurationMS = 5000,
                ForceNatWarningMessage = false
            },
            AISchemaUploadsSettings = new AISchemaUploadsSettings
            {
                OnStartCheckSchemaAndReUploadIfRequired = true,
                OnStartAlwaysUpdateAIProfile = true,
                UpdateUploadedAIProfileEveryNTicks = true,
                UpdateUploadedAIProfileEveryNTicks_NumTicks = 216000
            },
            FESettings = new FESettings
            {
                EnableOnlineTipsLoaderData = true,
                EnableFriendsLeaderboardData = true,
                TipsDataNumPagesWaitForOnlineData = 2,
                FriendsLeaderboardDataDelayMS = 2000
            },
            DLCSettings = new DLCSettings
            {
                ProductSettings = new List<ProductSetting>
                {
                    new ProductSetting
                    {
                        ProductID = "CCF0004",
                        RequiredEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "CCF00E0",
                        RequiredEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "CCF00F0",
                            EvenIfInstalled = true,
                            EvenIfPurchased = true
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "CCF00F0",
                        DisablingEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "CCF00E0",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "CCF000B",
                        RequiredEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "CCF000A",
                            EvenIfInstalled = true,
                            EvenIfPurchased = true
                        }
                    },
                    new ProductSetting
                    {
                        ProductID = "CCF000A",
                        DisablingEntitlement = new DLCItem
                        {
                            ItemID = "SKATE_SHARE_PACK",
                            EvenIfInstalled = false,
                            EvenIfPurchased = false
                        },
                        DisablingProduct = new DLCItem
                        {
                            ItemID = "CCF000B",
                            EvenIfInstalled = true,
                            EvenIfPurchased = true
                        }
                    }
                },
                InGameOnlyProducts = new List<string>
                {
                    "CCF0002",
                    "CCF0004",
                    "CCF00E0",
                    "CCF00F0",
                    "CCF000B",
                    "CCF000A"
                }
            }
        };
    }
}

[XmlRoot(ElementName = "Config")]
public class SkateConfig
{
    public ulong Reward { get; set; }

    public GameSettings GameSettings { get; set; }

    public MatchmakingSettings MatchmakingSettings { get; set; }

    public AISchemaUploadsSettings AISchemaUploadsSettings { get; set; }

    public FESettings FESettings { get; set; }

    [XmlElement("DLCSettings")]
    public DLCSettings DLCSettings { get; set; }
}


public class GameSettings
{
    public int OpenJoinTime0 { get; set; }
    public int OpenJoinTime1 { get; set; }
    public int OpenJoinTime2 { get; set; }
    public int OpenJoinTime3 { get; set; }
    public int OpenJoinTime4 { get; set; }
    public int OpenJoinTime5 { get; set; }
    public int LockedTime { get; set; }
    public int MaxGameTime { get; set; }
    public int HurryTime { get; set; }
    public int ResultsTime { get; set; }
    public int RewardsTime { get; set; }
    public int ChangeTime { get; set; }
    public int TimeoutMS { get; set; }
    public int ProposalTimeout { get; set; }
    public int MaxWaitForResetTime { get; set; }
    public int InactiveCountThreshold { get; set; }
    public decimal DeathRaceInactiveThreshold { get; set; }
}

public class MatchmakingSettings
{
    public bool AllowDLC { get; set; }
    public bool BypassDirtyCast { get; set; }
    public int CreateGameDurationMS { get; set; }
    public int FindGameDurationMS { get; set; }
    public bool ForceNatWarningMessage { get; set; }
}

public class AISchemaUploadsSettings
{
    public bool OnStartCheckSchemaAndReUploadIfRequired { get; set; }
    public bool OnStartAlwaysUpdateAIProfile { get; set; }
    public bool UpdateUploadedAIProfileEveryNTicks { get; set; }
    public int UpdateUploadedAIProfileEveryNTicks_NumTicks { get; set; }
}

public class FESettings
{
    public bool EnableOnlineTipsLoaderData { get; set; }
    public bool EnableFriendsLeaderboardData { get; set; }
    public int TipsDataNumPagesWaitForOnlineData { get; set; }
    public int FriendsLeaderboardDataDelayMS { get; set; }
}

public class DLCSettings
{
    [XmlElement("ProductSettings")]
    public List<ProductSetting> ProductSettings { get; set; }

    [XmlElement("InGameOnlyProduct")]
    public List<string> InGameOnlyProducts { get; set; }
}

public class ProductSetting
{
    public string ProductID { get; set; }

    public DLCItem RequiredEntitlement { get; set; }

    public DLCItem DisablingEntitlement { get; set; }

    public DLCItem DisablingProduct { get; set; }
}


public class DLCItem
{
    public string ItemID { get; set; }
    public bool EvenIfInstalled { get; set; }
    public bool EvenIfPurchased { get; set; }
}