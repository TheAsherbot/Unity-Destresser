using UnityEditor;

using UnityEngine;

namespace TheAshBot.Assets.Destresser
{
    public class DestresserEditorWindow : EditorWindow
    {


        [MenuItem("Tools/Destresser")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DestresserEditorWindow));
        }

        private static DestresserImageGenerator.Response response;

        private bool isFirstFrame = false;

        private async void OnEnable()
        {
            response = default(DestresserImageGenerator.Response);
            response = await DestresserImageGenerator.GetCatPicture();
        }

        private async void OnGUI()
        {
            if (response.picture == null)
            {
                isFirstFrame = true;
                EditorGUILayout.LabelField("Generating");
            }
            else
            {
                EditorGUILayout.LabelField("Images from Pexels. Photographer: " + response.photographer);
                if (EditorGUILayout.LinkButton("Image Link"))
                {
                    Application.OpenURL(response.url);
                }

                if (GUILayout.Button("Generate New Image"))
                {
                    response.picture = null;
                    response = await DestresserImageGenerator.GetCatPicture();
                }

                // For some reason when trying to access the image the first frame it is available it gives an null object reference.
                // This prevents the image from showing on first frame.
                if (!isFirstFrame)
                {
                    float windowAspect = (position.width - 10) / (position.height - 25);
                    float textureAspect = (float)response.picture.width / response.picture.height;

                    float scaledWidth;
                    float scaledHeight;

                    if (windowAspect > textureAspect) // Window is wider than the texture aspect ratio
                    {
                        scaledHeight = position.height - (25 / 1);
                        scaledWidth = scaledHeight * textureAspect;
                    }
                    else // Window is taller than the texture aspect ratio
                    {
                        scaledWidth = position.width - (15 / 1);
                        scaledHeight = scaledWidth / textureAspect;
                    }

                    EditorGUI.DrawPreviewTexture(new Rect(5, 60, scaledWidth, scaledHeight), response.picture);
                }
                isFirstFrame = false;
            }
        }


    }
}
