using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;

namespace vmp1r3.ScriptTemplate.Editor
{
	public sealed class Template
	{
		public Template(string icon, string name, string path)
		{
			Icon = icon;
			Name = name;
			Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(EditorApplication.applicationPath), "Data/Resources/ScriptTemplates", path);
			Content = File.ReadAllText(Path);
			UserEdits = Content;
		}

		public string Icon { get; }
		public string Name { get; }
		public string Path { get; }
		public string Content { get; }
		public string UserEdits { get; set; }
		public bool IsDirty => Content != UserEdits;

		[PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
		public void Save() => File.WriteAllText(Path, UserEdits);
		public void Open() => Process.Start(Path);
	}

	public static class ScriptTemplateEditorWindow
	{
		private static readonly Template[] templates = new Template[]
		{
			new Template("cs Script Icon", "C# Script", "81-C# Script-NewBehaviourScript.cs.txt"),
			new Template("cs Script Icon", "Test Script", "83-C# Script-NewTestScript.cs.txt"),
			new Template("cs Script Icon", "State Machine Behaviour", "86-C# Script-NewStateMachineBehaviourScript.cs.txt"),
			new Template("cs Script Icon", "Sub State Machine Behaviour", "86-C# Script-NewSubStateMachineBehaviourScript.cs.txt"),
			new Template("cs Script Icon", "Playable Behaviour", "87-Playables__Playable Behaviour C# Script-NewPlayableBehaviour.cs.txt"),
			new Template("cs Script Icon", "Playable Asset", "88-Playables__Playable Asset C# Script-NewPlayableAsset.cs.txt"),
			new Template("Shader Icon", "Unlit Shader", "84-Shader__Unlit Shader-NewUnlitShader.shader.txt"),
			new Template("Shader Icon", "Surface Shader", "83-Shader__Standard Surface Shader-NewSurfaceShader.shader.txt"),
			new Template("Shader Icon", "Image Effect Shader", "85-Shader__Image Effect Shader-NewImageEffectShader.shader.txt"),
			new Template("Shader Icon", "Compute Shader", "90-Shader__Compute Shader-NewComputeShader.compute.txt"),
			new Template("RayTracingShader Icon", "Raytracing Shader", "93-Shader__Ray Tracing Shader-NewRayTracingShader.raytrace.txt"),
			new Template("AssemblyDefinitionAsset Icon", "Assembly Definition", "91-Assembly Definition-NewAssembly.asmdef.txt"),
			new Template("AssemblyDefinitionAsset Icon", "Editor Test Assembly Definition", "92-Assembly Definition-NewEditModeTestAssembly.asmdef.txt"),
			new Template("AssemblyDefinitionAsset Icon", "Test Assembly Definition", "92-Assembly Definition-NewTestAssembly.asmdef.txt"),
			new Template("AssemblyDefinitionReferenceAsset Icon", "Assembly Definition Reference", "93-Assembly Definition Reference-NewAssemblyReference.asmref.txt"),
			new Template("SceneAsset Icon", "Scene Template Pipeline", "202-Scene Template Pipeline-NewSceneTemplatePipeline.cs.txt")
		};

		private static Template target;

		[PreferenceItem("Script Templates")]
		private static void CustomPreferencesGUI()
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(240));
				{
					foreach (var template in templates)
					{
						var style = new GUIStyle(EditorStyles.toolbarButton);
						style.alignment = TextAnchor.MiddleLeft;
						style.normal.textColor = template == target ? Color.white : style.normal.textColor;
						GUI.enabled = template != target;
						if (GUILayout.Button(EditorGUIUtility.TrTextContentWithIcon(template.Name, string.Empty, template.Icon), style, GUILayout.Width(240)))
						{
							if (target != null && target != template && target.IsDirty)
							{
								if (EditorUtility.DisplayDialog($"{target.Name} edited", $"Do you want to save changes in {target.Name}", "Yes", "No"))
									target.Save();
								else
									target.UserEdits = target.Content;
							}

							target = template;
						}
						GUI.enabled = true;
					}
				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
				{
					EditorGUI.BeginDisabledGroup(target == null);
					{
						if (target != null)
						{
							target.UserEdits = GUILayout.TextArea(target.UserEdits, GUILayout.ExpandHeight(true));

							GUILayout.Space(4);
							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							{
								if (GUILayout.Button("Open...", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
									target.Open();

								GUI.enabled = target.IsDirty;
								if (GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
									target.Save();
								GUI.enabled = true;
							}
							GUILayout.Space(8);
							GUILayout.EndHorizontal();
						}
						else
							GUILayout.Label("Select Template to modify", EditorStyles.boldLabel, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
						EditorGUI.EndDisabledGroup();
					}
				}
				GUILayout.Space(4);
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}
	}
}