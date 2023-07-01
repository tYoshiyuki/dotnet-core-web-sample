FROM nginx:latest

COPY ./nginx.conf /etc/nginx/nginx.conf

COPY ./site.conf.template /etc/nginx/templates/site.conf.template

CMD [ "nginx", "-g", "daemon off;" ]
