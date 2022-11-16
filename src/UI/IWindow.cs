using System.Collections.Generic;

public interface IWindow
{
    string GetId();

    IWindow GetActiveNestedWindow();

    void Rebuild();

    void OnReturn();

    List<UIDynamicSlider> GetSliders();

    void Clear();

    void ClosePopups();
}
