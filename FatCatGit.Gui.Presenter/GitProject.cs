﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FatCatGit.Gui.Presenter
{
    public class GitProject
    {
        private static readonly IEnumerable<string> ValidUrlProjects = new List<string>
                                                                           {
                                                                               "ssh",
                                                                               "git",
                                                                               "http",
                                                                               "ftp",
                                                                               "rsync",
                                                                               "file"
                                                                           };

        public GitProject(string projectUri)
        {
            ProjectUri = projectUri;

            DetermineGitProjectType();
        }

        protected string ProjectUri { get; set; }
        private GitProjectType ProjectType { get; set; }

        public string RepostoryName
        {
            get
            {
                if (ProjectType == GitProjectType.FolderLocation)
                {
                    return DetermineFolderRepositoryName();
                }

                return DetermineUriRepositoryName();
            }
        }

        public bool IsFolderLocation
        {
            get { return ProjectType == GitProjectType.FolderLocation; }
        }

        private void DetermineGitProjectType()
        {
            ProjectType = IsProjectTypeAUrl() ? GitProjectType.Url : GitProjectType.FolderLocation;
        }

        private bool IsProjectTypeAUrl()
        {
            return IsProjectUriValid() && ValidUrlProjects.Any(validUrlProject => ProjectUri.StartsWith(validUrlProject));
        }

        private string DetermineFolderRepositoryName()
        {
            if (!IsProjectUriValid())
            {
                return string.Empty;
            }

            var directoryInfo = new DirectoryInfo(ProjectUri);

            return directoryInfo.Name;
        }

        private bool IsProjectUriValid()
        {
            return !string.IsNullOrEmpty(ProjectUri);
        }

        private string DetermineUriRepositoryName()
        {
            if (!IsProjectUriValid())
            {
                return string.Empty;
            }

            // TODO:  Doing this with string indexs now.  Should look into regex eventually

            int index = ProjectUri.IndexOf(".git");

            int start = ProjectUri.LastIndexOf('/', index);

            if (start == -1)
            {
                start = ProjectUri.LastIndexOf(':', index);    
            }

            return ProjectUri.Substring(start + 1, index - start - 1);
        }

        private enum GitProjectType
        {
            Url,
            FolderLocation
        }
    }
}