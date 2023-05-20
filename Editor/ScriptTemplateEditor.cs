using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

namespace vmp1r3.ScriptTemplate.Editor
{
	public static class ScriptTemplateEditor
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
							target.Refresh();
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
							{
								if (GUILayout.Button("Reset to Default", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
									target.Reset();

								GUILayout.FlexibleSpace();
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

	public sealed class Template
	{
		public Template(string icon, string name, string fileName)
		{
			Icon = icon;
			Name = name;
			FileName = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), @"Data\Resources\ScriptTemplates", fileName);
			DefaultFile = Path.Combine(@"%APPDATA%\Unity\com.vmp1r3.script-template-editor\bak", fileName);
			Content = File.ReadAllText(FileName);
			UserEdits = Content;

			if (File.Exists(DefaultFile))
				return;

			if (!Directory.Exists(Path.GetDirectoryName(DefaultFile)))
				Directory.CreateDirectory(Path.GetDirectoryName(DefaultFile));

			File.WriteAllText(DefaultFile, Content);
		}

		public string Icon { get; }
		public string Name { get; }
		public string FileName { get; }
		public string DefaultFile { get; }
		public string Content { get; set; }
		public string UserEdits { get; set; }
		public bool IsDirty => Content != UserEdits;

		public void Open()
		{
			Process.Start(FileName);
		}

		public void Save()
		{
			string scriptPath = @"Assets/Editor/WriteFileAdmin.ps1"; // TODO: Access file from packages
			string command = $"-ExecutionPolicy Bypass -File \"{Path.Combine(Directory.GetCurrentDirectory(), scriptPath)}\" -FilePath \"{FileName}\" -Content \"{UserEdits}\"";

			Process process = new Process();
			process.StartInfo.FileName = "powershell.exe";
			process.StartInfo.Arguments = command;
			process.StartInfo.Verb = "runas";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.WaitForExit();

			Content = UserEdits;
		}

		public void Reset()
		{
			Content = UserEdits = File.ReadAllText(DefaultFile);
			Save();
		}

		public void Refresh()
		{
			UserEdits = Content;
		}
	}
}