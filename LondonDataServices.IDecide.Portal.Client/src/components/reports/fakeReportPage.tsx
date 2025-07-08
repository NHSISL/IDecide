import { FunctionComponent, useEffect } from "react"
import { Table } from "react-bootstrap"
import { PBIEvent } from "../../types/PBIEvent"

type FakeReportPageProps = {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    getEmbeddedComponent: (embedObject: any) => void
}

type EventCollection = {
    eventType: string,
    callback: (value?: PBIEvent) => void
}

type TestData = {
    testName: string,
    type: string,
    target: string,
    values: string[]
}


class FakeEmbedObject {

    events: EventCollection[] = []

    on(eventType: string, callback: (value?: PBIEvent) => void) {
        if (this.events.filter(x => x.eventType == eventType).length === 0) {
            this.events.push({ eventType, callback });
        }
    }

    processEvent(eventType: string, value?: PBIEvent) {
        console.log(this.events);
        this.events.filter(x => x.eventType == eventType).forEach(i => {
            i.callback(value);
        })
    }
}

const embedObject = new FakeEmbedObject();

const FakeReportPage: FunctionComponent<FakeReportPageProps> = ({ getEmbeddedComponent }) => {
    useEffect(() => {
        getEmbeddedComponent(embedObject);
        setTimeout(() => {
            embedObject.processEvent('loaded', undefined);
        }, 1000)
    }, []);

    const testDataTemplate = {
        detail: {
            dataPoints: [
                {
                    identity: [
                    ],
                    values: [
                    ]
                }
            ]
        }
    }

    const testData: TestData[] = [
        { testName: 'single identity column', type: 'identity', target: 'fake', values: ['1111151000'] },
        { testName: 'identity column with multiple records', type: 'identity', target: 'fake', values: ['1111192000', '1111209000'] },
        { testName: 'single value columnm', type: 'value', target: 'fake', values: ['1111129000'] },
        { testName: 'value column with multiple records', type: 'value', target: 'fake', values: ['1111131000', '1111112000'] },
        { testName: 'identity column without valid key', type: 'identity', target: 'notafake', values: ['1111128000', '1111209000'] },
        { testName: 'value column without valid column', type: 'value', target: 'notafake', values: ['1111137000', '1111200000'] },
    ]

    const test = (testData: TestData, value: string) => {
        if (testData.type === 'identity') {
            const event = JSON.parse(JSON.stringify(testDataTemplate));

            event.detail.dataPoints[0].identity.push({
                target: { column: testData.target },
                equals: value
            })
            embedObject.processEvent('dataSelected', event)
        }

        if (testData.type === 'value') {
            const event = JSON.parse(JSON.stringify(testDataTemplate));

            event.detail.dataPoints[0].values.push({
                target: {
                    measure: testData.target,
                },
                formattedValue: value,
                value: value
            })

            embedObject.processEvent('dataSelected', event)
        }
    }

    return <>

        {testData.map((td, i) => <div key={i}>
            <h4>{td.testName}</h4>
            <Table striped bordered hover>
                <tbody>
                    {td.values.map((val, index) => (
                        <tr key={`single-${index}`} onClick={() => test(td, val)}>
                            <td>Test single record click: {val}</td>
                        </tr>
                    ))}

                    {td.values.length > 1 && (
                        <tr key={`multiple-${i}`} onClick={() => test(td, td.values.join(','))}>
                            <td>Test multiple record click: {td.values.join(',')}</td>
                        </tr>
                    )}
                </tbody>
            </Table>
        </div>
        )
        }
    </>
}

export default FakeReportPage