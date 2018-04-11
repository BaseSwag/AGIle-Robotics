from twisted.web.server import Site
from twisted.web.static import File

webdir = File("../public")
site = Site(webdir)