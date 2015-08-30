using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.IO;

namespace Portal.Models.Galleries {
    public class GalleryFormModel {
        private string GetPath {
            get { return HttpContext.Current.Server.MapPath("~/public/userfiles/galleries") + "/"; }

        }

        private string GetPathHome
        {
            get { return HttpContext.Current.Server.MapPath("~/public/userfiles/gallery") + "/"; }

        }

      
        public DateTime Date { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string DefaultPhoto { get; set; }

        private IList<string> _deleteFile = new List<string>();
        public IList<string> DeleteFile {
            get {

                return _deleteFile;
            }
            set {
                _deleteFile = value;

                if (_deleteFile.Count > 0) {
                    foreach (var item in _deleteFile) {
                        File.Delete(GetPath + "large/" + Id + "/" + item);
                        File.Delete(GetPath + "medium/" + Id + "/" + item);
                        File.Delete(GetPathHome + item);
                        File.Delete(GetPath + "small/" + Id + "/" + item);
                      
                    }

                }
            }
        }

        private IList<HttpPostedFileBase> _uploadedFiles = new List<HttpPostedFileBase>();
        public IList<HttpPostedFileBase> UploadedFiles {
            get {
                return _uploadedFiles;
            }
            set {
                _uploadedFiles = value;
                CheckFolder();
                foreach (var item in _uploadedFiles) {
                    if (item != null && item.ContentLength > 0) {
                        var picture = Guid.NewGuid() + ".jpg";
                        var image = new WebImage(item.InputStream);

                        image.Resize(650, 360).Crop(1, 1).Save(GetPath + "large/" + Id + "/" + picture, "image/jpeg");
                        image.Resize(300, 180).Crop(1, 1).Save(GetPath + "medium/" + Id + "/" + picture, "image/jpeg");
                        image.Save(GetPathHome + picture, "image/jpeg");
                        image.Resize(100, 80).Crop(1, 1).Save(GetPath + "small/" + Id + "/" + picture, "image/jpeg");

                        if (string.IsNullOrEmpty(DefaultPhoto))
                            DefaultPhoto = picture;
                    }
                }
            }
        }

        private void CheckFolder() {
            if (!Directory.Exists(GetPath + "large/" + Id)) {
                Directory.CreateDirectory(GetPath + "large/" + Id);
                Directory.CreateDirectory(GetPath + "medium/" + Id);
                Directory.CreateDirectory(GetPath + "small/" + Id);
            }

        }
    }
}