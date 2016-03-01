using UnityEditor;
using System.Linq;

class WebGLBuilder
{
    static void build()
    {
        var scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
        BuildPipeline.BuildPlayer(scenes, "WebGL-Dist", BuildTarget.WebGL, BuildOptions.None);
    }
}
