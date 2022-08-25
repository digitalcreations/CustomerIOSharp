namespace CustomerIOSharp.Test;

using System;

public abstract class AuthorizationClass
{
    protected AuthorizationClass()
    {
        if (string.IsNullOrWhiteSpace(SiteId))
        {
            throw new Exception("CIOS_SITE_ID is missing from environment variables.");
        }

        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new Exception("CIOS_API_KEY is missing from environment variables.");
        }

        if (string.IsNullOrWhiteSpace(AppApiKey))
        {
            throw new Exception("CIOS_APP_API_KEY is missing from environment variables.");
        }

        if (SiteId == "SET_ME" || ApiKey == "SET_ME" || BroadcastCampaignId == 0 || BroadcastSegmentId == 0)
        {
            throw new Exception("Environment variables are set at default values, and should be updated. You need to set the environment variables in test.runsettings to run the tests locally.");
        }
    }

    public static string SiteId => Environment.GetEnvironmentVariable("CIOS_SITE_ID") ?? string.Empty;

    public static string ApiKey => Environment.GetEnvironmentVariable("CIOS_API_KEY") ?? string.Empty;

    public static string AppApiKey => Environment.GetEnvironmentVariable("CIOS_APP_API_KEY") ?? string.Empty;

    public static int BroadcastCampaignId => int.Parse(Environment.GetEnvironmentVariable("CIOS_BROADCAST_CAMPAIGN_ID"));

    public static int BroadcastSegmentId => int.Parse(Environment.GetEnvironmentVariable("CIOS_BROADCAST_SEGMENT_ID"));
}