namespace LibMain.Model.File {
    public enum FileStatusEnum
    {
        Untracked,
        Modified,
        Added,
        Deleted,
        Renamed,
        Copied,
        UpdatedButUnmerged,
        Unmodified,
        Ignored,
        Conflicted,
        Unknown
    }
}
