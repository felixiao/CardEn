import wsgiref.handlers
from PIL import Image
import webapp2
from google.appengine.ext import webapp
from google.appengine.ext.webapp \
	import template

class MainPage(webapp2.RequestHandler):
  def get(self):
	user = users.get_current_user()
	if user:
        self.response.headers['Content-Type'] = 'text/plain'
        self.response.out.write('Hello, ' + user.nickname())
    else:
        self.redirect(users.create_login_url(self.request.uri))

application = webapp2.WSGIApplication([('/', MainPage),('/page1',page1)],debug=True)
def main():
    application.run()

if __name__ == "__main__":
    main()