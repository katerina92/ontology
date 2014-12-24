ontology
========
Почтовые отделения

Каждому почтовому отделению соответствует уникальный почтовый индекс.

1) Исходные данные 

Источник: http://basicdata.ru/download/zipcode/
Файл с исходными данными: data.xlsx

2) Данные в формате turtle: test_full.ttl

Пример ресурса в rdf-формате:



3) Используемые онтологии: Schema: http://schema.org/

4) Примеры sparql запросов: Query

Настройка фусеки и пабби:
качаем фусеки по инструкции - https://github.com/ailabitmo/semweb-course/tree/master/example-1
далее, кидаем файл test_full.ttl в папку с фусеки и вводим в терминале:
./fuseki-server --file=test_full.ttl /ds

после этого надо скачать и настроить пабби:
качаем и устанавливаем по инструкции http://wifo5-03.informatik.uni-mannheim.de/pubby/
после чего редактируем config.ttl (root/WEB-INF/) как указано ниже 

# Pubby Example Configuration
#
# This configuration connects to the DBpedia SPARQL endpoint and
# re-publishes on your local machine, with dereferenceable
# localhost URIs.
#
# This assumes you already have a servlet container running
# on your machine at http://localhost:8080/ .
#
# Install Pubby as the root webapp of your servlet container,
# and make sure the config-file parameter in Pubby's web.xml
# points to this configuration file.
#
# Then browse to http://localhost:8080/ .

# Prefix declarations to be used in RDF output
@prefix conf: <http://richard.cyganiak.de/2007/pubby/config.rdf#> .
@prefix meta: <http://example.org/metadata#> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix owl: <http://www.w3.org/2002/07/owl#> .
@prefix dc: <http://purl.org/dc/elements/1.1/> .
@prefix dcterms: <http://purl.org/dc/terms/> .
@prefix foaf: <http://xmlns.com/foaf/0.1/> .
@prefix skos: <http://www.w3.org/2004/02/skos/core#> .
@prefix geo: <http://www.w3.org/2003/01/geo/wgs84_pos#> .
@prefix dbpedia: <http://localhost:8080/resource/> .
@prefix p: <http://localhost:8080/property/> .
@prefix yago: <http://localhost:8080/class/yago/> .
@prefix units: <http://dbpedia.org/units/> .
@prefix geonames: <http://www.geonames.org/ontology#> .
@prefix prv:      <http://purl.org/net/provenance/ns#> .
@prefix prvTypes: <http://purl.org/net/provenance/types#> .
@prefix doap:     <http://usefulinc.com/ns/doap#> .
@prefix void:     <http://rdfs.org/ns/void#> .
@prefix ir:       <http://www.ontologydesignpatterns.org/cp/owl/informationrealization.owl#> .

# Server configuration section
<> a conf:Configuration;
conf:projectName "DBpedia.org";
conf:projectHomepage <http://localhost:8080>;
conf:webBase <http://localhost:8080/>;
conf:usePrefixesFrom <>;
conf:webResourcePrefix "resource/";
conf:defaultLanguage "en";
conf:indexResource <http://localhost:8080/resource/index/101000>;
conf:dataset [
conf:sparqlEndpoint <http://localhost:3030/ds/query>;
#conf:sparqlDefaultGraph <http://localhost:3030/>;
conf:datasetBase <http://localhost:8080/resource/>;
conf:webResourcePrefix "resource/";
conf:fixUnescapedCharacters "(),'!$&*+;=@";
];
.


запускаем пабби командой:
java -jar start.jar
