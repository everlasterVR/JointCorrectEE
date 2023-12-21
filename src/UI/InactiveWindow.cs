sealed class InactiveWindow : WindowBase
{
    public InactiveWindow(JointCorrectEE script) : base(script, nameof(InactiveWindow))
    {
    }

    protected override void OnBuild()
    {
        AddInfoTextField("<b>GameObject is inactive, likely due to the atom being disabled.</b>", false);
    }
}
