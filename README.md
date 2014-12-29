ontology
========
Почтовые отделения

Каждому почтовому отделению соответствует уникальный почтовый индекс.

1) Исходные данные 

Источник: http://basicdata.ru/download/zipcode/
Файл с исходными данными: data.xlsx

2) Данные в формате turtle: test_full.ttl

3) Пример ресурса в rdf-формате: exmpl

4) Используемые онтологии: Schema: http://schema.org/

5) Примеры sparql запросов: Query

6) Настройка фусеки и пабби:
качаем фусеки по инструкции - https://github.com/ailabitmo/semweb-course/tree/master/example-1
далее, кидаем файл test_full.ttl в папку с фусеки и вводим в терминале:
./fuseki-server --file=test_full.ttl /ds

после этого надо скачать и настроить пабби:
качаем и устанавливаем по инструкции http://wifo5-03.informatik.uni-mannheim.de/pubby/
после чего редактируем config.ttl (root/WEB-INF/) в соответствии с конфигом, который приложен в репозитории


запускаем пабби командой:
java -jar start.jar
