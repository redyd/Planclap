import org.gradle.api.DefaultTask
import org.gradle.api.file.FileCollection
import org.gradle.api.tasks.InputFiles
import org.gradle.api.tasks.OutputFile
import org.gradle.api.tasks.TaskAction
import java.io.File
import java.nio.charset.Charset
import javax.xml.parsers.DocumentBuilderFactory

abstract class AggregatePmdReportsTask : DefaultTask() {
    @get:InputFiles
    abstract var projectsFiles: FileCollection;

    @get:OutputFile
    abstract var reportFile: File;

    @TaskAction
    fun aggregate() {
        println("Aggregating pmd reports")
        val aggregatedRules = HashMap<String, ArrayList<Map<String,String>>>();

        projectsFiles.forEach { it
            println("Inspecting ${it.path}")
            val document = DocumentBuilderFactory.newInstance().newDocumentBuilder().parse(it);

            val nodeList = document.getElementsByTagName("file")
            for (i in 0 until nodeList.length) {
                val fileNode = nodeList.item(i)
                val childNodes = fileNode.childNodes;
                for (j in 0 until childNodes.length) {
                    val violationNode = childNodes.item(j)
                    if(violationNode.nodeName.equals("violation")) {
                        val attributes = violationNode.attributes;
                        val rule = attributes.getNamedItem("rule").nodeValue;
                        val values = HashMap<String, String>();
                        values.put("class", attributes.getNamedItem("class").nodeValue)
                        values.put("beginline", attributes.getNamedItem("beginline").nodeValue)
                        values.put("endline", attributes.getNamedItem("endline").nodeValue)
                        values.put("text", violationNode.textContent)
                        if(!aggregatedRules.containsKey(rule)) {
                            aggregatedRules.put(rule, ArrayList());
                        }
                        val violationsForRule = aggregatedRules.get(rule);
                        violationsForRule!!.add(values);
                    }
                }
            }
        }

        var documentResult = """
            <html>
              <head>
                <meta name='viewport' content='width=device-width, initial-scale=1' />
                <link rel='stylesheet' href='https://www.w3schools.com/w3css/4/w3.css' />
              </head>
              <body id='main'>
                <div class='w3-container'>
        """.trimIndent();
        if(aggregatedRules.isNotEmpty()) {
            documentResult += aggregatedRules.map { entries ->
                """
              <div>
              <h2>${entries.key} (violation count: ${entries.value.size})</h2>
              """.trimIndent() +
                        entries.value.map { s ->
                            """
                <div>
                  <strong>Type: ${s.get("class")} begins at ${s.get("beginline")} ends at ${s.get("endline")}</strong>
                  <p>${s.get("text")}</p>
                </div>
            """.trimIndent()
                        }.reduce { a, b -> "$a\n$b" } +
                        """
              </div>
            """.trimIndent()
            }.reduce { a, b -> "$a\n$b" };
        } else {
            documentResult+="<div><h2>There are 0 Pmd Violation in your projects</h2></div>\n"
        }
        documentResult+="""
                </div>
              </body>
            </html>
        """.trimIndent();
        reportFile.writeText(documentResult, Charset.forName("utf-8"))
    }
}